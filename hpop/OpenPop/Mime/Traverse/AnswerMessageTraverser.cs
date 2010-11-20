using System.Collections.Generic;

namespace OpenPop.Mime.Traverse
{
	/// <summary>
	/// This is an abstract class which handles traversing of a <see cref="Message"/> tree structure.
	/// It runs through the message structure using a depth-first traversal.
	/// </summary>
	/// <typeparam name="A">The answer you want from traversing the message tree structure</typeparam>
	public abstract class AnswerMessageTraverser<A> : IAnswerMessageTraverser<A>
	{
		/// <summary>
		/// Call this when you want an answer for a full message.
		/// </summary>
		/// <param name="message">The message you want to traverse</param>
		/// <returns>An answer</returns>
		public A VisitMessage(Message message)
		{
			return VisitMessagePart(message.MessagePart);
		}

		/// <summary>
		/// Call this method when you want to find an answer for a <see cref="MessagePart"/>
		/// which is a <see cref="MessagePart.IsMultiPart">MultiPart</see> message.
		/// </summary>
		/// <param name="messagePart">The message part you want an answer from. Must be a MultiPart message.</param>
		/// <returns>An answer</returns>
		public A VisitMessagePart(MessagePart messagePart)
		{
			if(messagePart.IsMultiPart)
			{
				List<A> leafAnswers = new List<A>(messagePart.MessageParts.Count);
				foreach (MessagePart part in messagePart.MessageParts)
				{
					leafAnswers.Add(VisitMessagePart(part));
				}
				return MergeLeafAnswers(leafAnswers);
			}

			return CaseLeaf(messagePart);
		}

		/// <summary>
		/// For a concrete implementation an answer must be returned for a leaf <see cref="MessagePart"/>, which are
		/// MessageParts that are not <see cref="MessagePart.IsMultiPart">MultiParts.</see>
		/// </summary>
		/// <param name="messagePart">The message part which is a leaf and thereby not a MultiPart</param>
		/// <returns>An answer</returns>
		protected abstract A CaseLeaf(MessagePart messagePart);

		/// <summary>
		/// For a concrete implementation, when a MultiPart <see cref="MessagePart"/> has fetched it's answers from it's children, these
		/// answers needs to be merged. This is the responsibility of this method.
		/// </summary>
		/// <param name="leafAnswers">The answer that the leafs gave</param>
		/// <returns>A merged answer</returns>
		protected abstract A MergeLeafAnswers(List<A> leafAnswers);
	}
}